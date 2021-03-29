const express = require('express')
const KafkaProducer = require('../../Tools/KafkaProducer')
KafkaProducer.connect();
const app = express();

app.get('/', (req, res) => {
  KafkaProducer.produceMessage("topic1", "Message from service1");
  res.send('Nodejs says hello world');
})

app.listen(80, () => {
  console.log(`Example app listening at http://localhost:80`)
})
