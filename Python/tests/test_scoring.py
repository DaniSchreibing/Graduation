from logic.scoring import ConfigurableScoring
from logic.models import Barn, FlockBatch
from datetime import date

def test_scoring_uses_config(tmp_path):
    config_file = tmp_path / "scoring.csv"
    config_file.write_text(
        "key,value\n"
        "Capacity Weight,1\n"
        "Region Penalty,50\n"
        "Housing Penalty,1000\n"
        "Region Multiplier,2\n"
        "Housing Multiplier,10\n"
    )
    scoring = ConfigurableScoring(str(config_file))
    barn = Barn("B1", 1000, "CAGE", "NL")
    flock = FlockBatch("F1", 400, "CAGE", date(2025, 3, 1), date(2025, 3, 10), "NL")

    score = scoring.score(barn, flock, remaining_capacity=1000)

    # Score = |remaining - size| * capacity_weight = |1000-400|*1 = 600
    assert score == 600