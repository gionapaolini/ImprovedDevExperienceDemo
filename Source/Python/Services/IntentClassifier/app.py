from flask import Flask, request
from kafka import KafkaConsumer
import os
import json
import uuid
from threading import Thread
from sentence_transformers import SentenceTransformer, util
from pymongo import MongoClient

app = Flask(__name__)
db = MongoClient(os.environ['MONGODB_SERVER_URL']).demoDB.intentprojectdatas
class IntentMachine:
    def __init__(self):
      self.model = SentenceTransformer('paraphrase-distilroberta-base-v1')
      self.intentData = dict()
    def updateData(self, projectId):
      data = db.find_one({'projectId': projectId})['data']
      flattened_data =  list(zip(*[(sentence, item["intent"]) for item in data for sentence in item["sentences"]]))
      sentences = flattened_data[0]
      intents = flattened_data[1]
      embeddings = self.model.encode(sentences)
      self.intentData[projectId] = dict()
      self.intentData[projectId]["sentences"] = sentences
      self.intentData[projectId]["intents"] = intents
      self.intentData[projectId]["embeddings"] = embeddings
      
    def getIntent(self, projectId, question):
      embedded_question = self.model.encode(question)
      projectData = self.intentData[projectId]
      cosine_scores = util.pytorch_cos_sim(projectData["embeddings"], embedded_question)
      sortedResult = sorted(list(zip(projectData["intents"], cosine_scores)), key=lambda x: x[1], reverse = True)
      confidence = sortedResult[0][1].item()
      if confidence > 0.5: return {"intent": sortedResult[0][0], "confidence": confidence}
      return "No intent was found"
      
intentMachine = IntentMachine()

@app.route('/<projectId>', methods=['GET'])
def getIntent(projectId):
  return intentMachine.getIntent(projectId, request.args.get('question'))

def consume():
  consumer = KafkaConsumer('intents-updated', group_id=os.environ['KAFKA_CONSUMER_GROUP'], client_id=os.environ['KAFKA_CLIENT_ID']+str(uuid.uuid4()),bootstrap_servers=[os.environ['KAFKA_SERVER_URL']])
  for msg in consumer:
    try:
      intentMachine.updateData(json.loads(msg.value)["projectId"])
    except: 
      print("An exception occurred") 

Thread(target=consume).start()

app.run(host="0.0.0.0", port="80")
