namespace S13Audio;

public interface IEnvelopeDetection
{
	float[] Buffer { get; set; }

	float this[int index] { get; }

	void Reset();
}
