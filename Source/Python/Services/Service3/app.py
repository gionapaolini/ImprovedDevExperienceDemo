from flask import Flask
from kafka import KafkaConsumer
import os
from multiprocessing import Process

app = Flask(__name__)

@app.route('/')
def hello_world():
    return 'Hello, World14!'

def consume():
    consumer = KafkaConsumer('topic1', group_id=os.environ['KAFKA_CONSUMER_GROUP'], client_id=os.environ['KAFKA_CLIENT_ID'],bootstrap_servers=[os.environ['KAFKA_SERVER_URL']])
    for msg in consumer:
        print (msg)
print("CIAO123")

Process(target=consume).start()
print("CIAO")

app.run(host="0.0.0.0", port="80")
