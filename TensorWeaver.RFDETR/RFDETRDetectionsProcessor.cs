using Microsoft.ML.OnnxRuntime.Tensors;
using TensorWeaver.OutputData;
using TensorWeaver.OutputProcessing;

namespace TensorWeaver.RFDETR;

public sealed class RFDETRDetectionsProcessor : OutputProcessor<List<Detection>>
{
	public float MinimumConfidence
	{
		get;
		set
		{
			if (value is <= 0 or >= 1)
			{
				var message = $"Value for {MinimumConfidence} should be exclusively between 0 and 1, but was {value}";
				throw new ArgumentOutOfRangeException(nameof(MinimumConfidence), value, message);
			}
			field = value;
		}
	} = 0.5f;

	public List<Detection> Process(RawOutput output)
	{
		var detections = new List<Detection>();
		Process(output, detections);
		return detections;
	}

	public void Process(RawOutput output, IList<Detection> target)
	{
		var boxes = output.Tensors[0];
		var logits = output.Tensors[1];
		var queriesCount = boxes.Dimensions[1];
		for (int queryIndex = 0; queryIndex < queriesCount; queryIndex++)
		{
			var classification = GetClassification(logits, queryIndex);
			if (classification.Confidence < MinimumConfidence)
				continue;
			var bounding = GetBounding(boxes, queryIndex);
			var detection = new Detection(classification, bounding, (ushort)queryIndex);
			target.Add(detection);
		}
	}

	private static Classification GetClassification(DenseTensor<float> tensor, int queryIndex)
	{
		var classesCount = tensor.Dimensions[2];
		const int batchIndex = 0;
		var mostConfidentClassification = new Classification(0, tensor[[batchIndex, queryIndex, 0]]);
		for (int classId = 1; classId < classesCount; classId++)
		{
			var confidence = tensor[batchIndex, queryIndex, classId];
			if (confidence > mostConfidentClassification.Confidence)
				mostConfidentClassification = new Classification((ushort)classId, confidence);
		}
		mostConfidentClassification = SigmoidConfidence(mostConfidentClassification);
		return mostConfidentClassification;
	}

	private static Bounding GetBounding(DenseTensor<float> tensor, int queryIndex)
	{
		const int batchIndex = 0;

		var xCenter = tensor[[batchIndex, queryIndex, 0]];
		var yCenter = tensor[[batchIndex, queryIndex, 1]];
		var width = tensor[[batchIndex, queryIndex, 2]];
		var height = tensor[[batchIndex, queryIndex, 3]];

		return Bounding.FromPoint(xCenter, yCenter, width, height);
	}

	private static Classification SigmoidConfidence(Classification classification)
	{
		var classId = classification.ClassId;
		var confidence = classification.Confidence;
		confidence = Sigmoid(confidence);
		return new Classification(classId, confidence);
	}

	private static float Sigmoid(float value)
	{
		return 1f / (1f + MathF.Exp(-value));
	}
}