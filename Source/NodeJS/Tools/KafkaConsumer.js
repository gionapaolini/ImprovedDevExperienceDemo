const kafka = require("./Kafka");

const consumer = kafka.consumer({ groupId: process.env.KAFKA_CONSUMER_GROUP });

const KafkaConsumer = {
  initializeAndStartConsumer: async (topics) => {
    await consumer.connect().catch((e) => {
      console.error(e);
    });

    for (var topicName in topics) {
      await consumer.subscribe({ topic: topicName, fromBeginning: true });
    }
    await consumer.run({
      eachMessage: async ({ topic, message }) => {
        if (topics[topic]) {
          topics[topic](JSON.parse(message.value)).catch((err) => {
            console.error(err);
          });
        }
      },
    });
  },
};

module.exports = KafkaConsumer;
