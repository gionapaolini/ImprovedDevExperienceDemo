from flask import Flask, request
from kafka import KafkaConsumer
import os
import json
import uuid
from threading import Thread
from transformers import pipeline
from pymongo import MongoClient

app = Flask(__name__)
db = MongoClient(os.environ['MONGODB_SERVER_URL']).demoDB.qamodels

class AnsweringMachine:
    def __init__(self):
      self.qa = pipeline("question-answering")
      self.context = dict()
    def updateContext(self, projectId):
      self.context[projectId] = db.find_one({'projectId': projectId})['data']
    def answerQuestion(self, projectId, question):
      return self.qa(question=question, context=self.context[projectId])

answeringMachine = AnsweringMachine()

@app.route('/<projectId>', methods=['GET'])
def answer(projectId):
  return answeringMachine.answerQuestion(projectId, request.args.get('question'))

def consume():
  consumer = KafkaConsumer('qa-updated', group_id=os.environ['KAFKA_CONSUMER_GROUP'], client_id=os.environ['KAFKA_CLIENT_ID']+str(uuid.uuid4()),bootstrap_servers=[os.environ['KAFKA_SERVER_URL']])
  for msg in consumer:
    answeringMachine.updateContext(json.loads(msg.value)["projectId"])

Thread(target=consume).start()

app.run(host="0.0.0.0", port="80")
