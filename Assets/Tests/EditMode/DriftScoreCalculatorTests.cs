using CipherDrift.Drift;
using NUnit.Framework;

public class DriftScoreCalculatorTests
{
    [Test]
    public void NoDriftProducesNoPoints()
    {
        var scorer = new DriftScoreCalculator();
        int score = scorer.Evaluate(2f, 40f, 1f, 0.02f);
        Assert.AreEqual(0, score);
    }

    [Test]
    public void SustainedDriftProducesPoints()
    {
        var scorer = new DriftScoreCalculator();
        int score = scorer.Evaluate(25f, 65f, 0.8f, 0.02f);
        Assert.Greater(score, 0);
    }

    [Test]
    public void ComboDoesNotExceedFive()
    {
        var scorer = new DriftScoreCalculator();
        for (int i = 0; i < 1000; i++)
            scorer.Evaluate(30f, 80f, 1f, 0.1f);

        Assert.LessOrEqual(scorer.Combo, 5f);
    }
}
