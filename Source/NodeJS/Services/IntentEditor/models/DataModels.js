const mongoose = require("mongoose");

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

const IntentProjectData = mongoose.model("IntentProjectData", IntentProjectDataSchema);

module.exports = {
    IntentProjectData
}
