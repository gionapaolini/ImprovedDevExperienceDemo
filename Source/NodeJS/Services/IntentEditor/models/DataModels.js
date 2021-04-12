const mongoose = require("mongoose");

const QAModelSchema = new mongoose.Schema(
    {
        projectId: {
            type: String,
            index: true,
            required: true,
            unique: true,
        },
        data: {
            type: String,
            required: true,
            minlength: 10,
            maxlength: 1000
        }
    },
);

const IntentProjectDataSchema = new mongoose.Schema(
    {
        projectId: {
            type: String,
            index: true,
            required: true,
            unique: true,
        },
        data: [
            {
                intent: {
                    type: String,
                    required: true,
                },
                sentences: [String]
            }
        ]
    },
);

const QAModel = mongoose.model("QAModel", QAModelSchema);
const IntentProjectData = mongoose.model("IntentProjectData", IntentProjectDataSchema);

module.exports = {
    QAModel, IntentProjectData
}
