from logic.loaders import load_barns, load_flocks
from datetime import date
import pytest

def test_load_barns(tmp_path):
    barns_csv = tmp_path / "barns.csv"
    barns_csv.write_text(
        "Name,Capacity,Housing Type,Region\n"
        "B1,1000,CAGE,NL\n"
        "B2,800,FLOOR,DE\n"
    )

    barns = load_barns(str(barns_csv))

    assert len(barns) == 2
    assert barns[0].id == "B1"
    assert barns[0].capacity == 1000
    assert barns[1].housing_type == "FLOOR"


def test_load_flocks(tmp_path):
    flocks_csv = tmp_path / "flocks.csv"
    flocks_csv.write_text(
        "id,size,housing_type,target_start,target_end,region\n"
        "F1,400,CAGE,2025-03-01,2025-03-10,NL\n"
        "F2,300,FLOOR,2025-03-05,2025-03-20,DE\n"
    )

    flocks = load_flocks(str(flocks_csv))

    assert len(flocks) == 2
    assert flocks[0].id == "F1"
    assert flocks[0].size == 400
    assert flocks[0].housing_type == "CAGE"
    assert flocks[0].target_start == date(2025, 3, 1)
    assert flocks[1].region == "DE"

def test_load_barns_missing_columns(tmp_path):
    barns_csv = tmp_path / "barns.csv"
    barns_csv.write_text("invalid,columns\nX,Y\n")

    with pytest.raises(KeyError):
        load_barns(str(barns_csv))

def test_load_flocks_missing_columns(tmp_path):
    flocks_csv = tmp_path / "flocks.csv"
    flocks_csv.write_text("bad,header\n1,2\n")

    with pytest.raises(KeyError):
        load_flocks(str(flocks_csv))