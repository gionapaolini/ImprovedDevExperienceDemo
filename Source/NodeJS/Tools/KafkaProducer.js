const kafka = require("./Kafka");
const producer = kafka.producer();

const KafkaProducer = {
  connect: async () => {
    await producer.connect().catch((e) => {console.error(e);});
    this.connected = true;
  },
  produceMessage: async (topic, message) => {
    if (!this.connected) throw new Error("KafkaProducer not connected");
    await producer.send({
      topic: topic,
      messages: [{ value: JSON.stringify(message) }],
    });
  },
};

module.exports = KafkaProducer;
