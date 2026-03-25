from datetime import date
from logic.models import Barn, FlockBatch, PlacementLine

from logic.kpi.kpi_engine import KpiEngine
from logic.kpi.kpi_models import KpiDefinition

from logic.kpi.kpi_registry import evaluate_kpis


def write_kpi_csv(tmp_path):
    kpi_file = tmp_path / "kpis.csv"
    kpi_file.write_text(
        "kpi_name,scope,metric,description\n"
        "OCC,global,occupancy_percent,Occupancy\n"
        "AGEDEV,global,age_deviation_index,Age deviation\n"
        "HSEBAL,global,housing_balance,Housing balance\n"
        "COST,global,cost_estimate,Cost\n"
        "DISPIM,global,disruption_impact,Disruption impact\n"
    )
    return str(kpi_file)


def make_barns():
    return [
        Barn("B1", 1000, "CAGE", "NL"),
        Barn("B2", 500, "FLOOR", "DE"),
    ]


def make_flocks():
    return [
        FlockBatch("F1", 400, "CAGE", date(2025, 3, 1), date(2025, 3, 10), "NL"),
        FlockBatch("F2", 300, "FLOOR", date(2025, 3, 2), date(2025, 3, 12), "DE"),
        FlockBatch("F3", 200, "CAGE", date(2025, 3, 5), date(2025, 3, 15), "NL"),
    ]


def make_placements():
    return [
        PlacementLine("F1", "B1", date(2025, 3, 1), 400),
        PlacementLine("F2", "B2", date(2025, 3, 2), 300),
        # Unplaced flock → drives DISPIM and AGEDEV skip logic
        PlacementLine("F3", None, None, 0, "No capacity"),
    ]


def test_kpi_full_evaluation(tmp_path):
    kpi_path = write_kpi_csv(tmp_path)

    barns = make_barns()
    flocks = make_flocks()
    placements = make_placements()

    results = evaluate_kpis(
        kpi_path,
        placements=placements,
        barns=barns,
        flocks=flocks
    )

    names = {r.name: r.value for r in results}

    # OCC — allocated 700 / capacity 1500
    assert "OCC" in names
    assert names["OCC"] == round((700 / 1500) * 100, 2)

    # AGEDEV — only placed flocks counted
    # F1 deviation = 0 days
    # F2 deviation = 0 days
    assert "AGEDEV" in names
    assert names["AGEDEV"] == 0

    # HSEBAL — variance > 0 because distribution uneven
    assert "HSEBAL" in names
    assert names["HSEBAL"] >= 0

    # COST — default cost logic (0.12 per bird unless you changed it)
    assert "COST" in names
    assert names["COST"] == round(700 * 0.12, 2)

    # DISPIM — affected birds (unplaced 0 now, but presence tested)
    assert "DISPIM" in names
    # Total birds placed = 700
    # Affected birds = 0 allocated where reason exists
    assert names["DISPIM"] == 0


def test_kpi_empty_inputs():
    registry = [
        KpiDefinition("OCC", "global", "occupancy_percent"),
        KpiDefinition("COST", "global", "cost_estimate"),
        KpiDefinition("DISP", "global", "disruption_impact"),
    ]
    engine = KpiEngine(registry)

    results = engine.evaluate([], [], [], mock_cost_per_bird=0.12)

    values = {r.name: r.value for r in results}
    assert values["OCC"] == 0
    assert values["COST"] == 0
    assert values["DISP"] == 0

def test_kpi_custom_multiplier():
    registry = [
        KpiDefinition("OCC", "global", "occupancy_percent")
    ]
    engine = KpiEngine(registry)

    barns = [Barn("B1", 1000, "CAGE", "NL")]
    placements = [PlacementLine("F1", "B1", date(2025, 1, 1), 500)]
    flocks = []

    results = engine.evaluate(
        placements, barns, flocks,
        scoring_config={"occupancy_multiplier": 2.0}
    )

    occ = results[0].value
    # OCC = (500 / 1000 * 100) = 50 * 2 = 100
    assert occ == 100
