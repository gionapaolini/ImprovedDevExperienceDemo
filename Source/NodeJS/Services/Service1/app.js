const express = require('express')
const KafkaProducer = require('../../Tools/KafkaProducer')
var bodyParser = require('body-parser')
const mongoose = require("mongoose");
const { QAModel, IntentProjectData } = require('./models/DataModels');

mongoose.connect(process.env.MONGODB_SERVER_URL, {
  useNewUrlParser: true,
  dbName: "demoDB",
  useUnifiedTopology: true,
  useCreateIndex: true,
});

KafkaProducer.connect();

const app = express();
app.use(bodyParser.json())

app.post('/qa/:projectId', async (req, res) => {
  const { projectId } = req.params;
  const { data } = req.body;
  await QAModel.updateOne({projectId}, {data}, { upsert: true });
  KafkaProducer.produceMessage("qa-updated", { projectId });
  res.status(200).json();
})

app.post('/intents/:projectId', async (req, res) => {
  const { projectId } = req.params;
  const { data } = req.body;
  await IntentProjectData.updateOne({projectId}, {data}, { upsert: true });
  KafkaProducer.produceMessage("intents-updated", { projectId });
  res.status(200).json();
})

app.get('/qa/:projectId', async (req, res) => {
  const { projectId } = req.params;
  const data = await QAModel.findOne({projectId});
  res.status(200).json(data);
})

app.get('/intents/:projectId', async (req, res) => {
  const { projectId } = req.params;
  const data = await IntentProjectData.findOne({projectId});
  res.status(200).json(data);
})

app.listen(80, () => {
  console.log(`Example app listening at http://localhost:80`)
})
