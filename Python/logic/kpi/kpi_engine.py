from typing import Dict, List, Optional
from statistics import variance
from datetime import date
from ..models import PlacementLine, Barn, FlockBatch
from .kpi_models import KpiDefinition, KpiResult


class KpiEngine:
    def __init__(self, registry: List[KpiDefinition]):
        self.registry = registry

    def evaluate(
        self,
        placements: List[PlacementLine],
        barns: List[Barn],
        flocks: List[FlockBatch],
        mock_cost_per_bird: float = 0.12,
        scoring_config: Optional[Dict[str, float]] = None
    ) -> List[KpiResult]:

        def normalize_key(raw_key: str) -> str:
            return str(raw_key).strip().lower().replace(" ", "_")

        normalized_config = {
            normalize_key(key): value
            for key, value in (scoring_config or {}).items()
        }

        def config_value(aliases: List[str], default: float) -> float:
            for alias in aliases:
                normalized_alias = normalize_key(alias)
                if normalized_alias in normalized_config:
                    return normalized_config[normalized_alias]
            return default

        cost_per_bird = config_value(
            aliases=["cost_per_bird", "kpi_cost_per_bird", "mock_cost_per_bird"],
            default=mock_cost_per_bird
        )
        occupancy_multiplier = config_value(
            aliases=["occupancy_multiplier", "kpi_occupancy_multiplier", "occupancy_percent_multiplier"],
            default=1.0
        )
        age_deviation_multiplier = config_value(
            aliases=["age_deviation_multiplier", "kpi_age_deviation_multiplier", "age_deviation_index_multiplier"],
            default=1.0
        )
        housing_balance_multiplier = config_value(
            aliases=["housing_balance_multiplier", "kpi_housing_balance_multiplier"],
            default=1.0
        )
        disruption_impact_multiplier = config_value(
            aliases=["disruption_impact_multiplier", "kpi_disruption_impact_multiplier"],
            default=1.0
        )

        results: List[KpiResult] = []

        # -----------------------------
        # Common aggregates
        # -----------------------------
        total_capacity = sum(b.capacity for b in barns)
        total_allocated = sum(p.allocated_qty for p in placements if p.barn_id)

        barn_usage = {}
        for p in placements:
            if p.barn_id:
                barn_usage[p.barn_id] = barn_usage.get(p.barn_id, 0) + p.allocated_qty

        # housing distribution
        housing_actual = {}
        barn_lookup = {b.id: b for b in barns}

        for p in placements:
            if p.barn_id:
                h = barn_lookup[p.barn_id].housing_type
                housing_actual[h] = housing_actual.get(h, 0) + p.allocated_qty

        total_birds = sum(housing_actual.values())

        for kpi in self.registry:

            # OCC — Occupancy %
            if kpi.metric == "occupancy_percent":
                value = ((total_allocated / total_capacity * 100) if total_capacity > 0 else 0) * occupancy_multiplier

                results.append(KpiResult(
                    name=kpi.name,
                    scope="global",
                    value=round(value, 2),
                    description=kpi.description
                ))

            # AGEDEV — Age Deviation Index
            elif kpi.metric == "age_deviation_index":
                deviations = []

                flock_map = {f.id: f for f in flocks}

                for p in placements:
                    if not p.barn_id:
                        continue  # skip unplaced
                    if not p.planned_start:
                        continue  # skip missing date
                    
                    flock = flock_map[p.flock_id]
                    deviation = abs((p.planned_start - flock.target_start).days)
                    deviations.append(deviation)

                value = sum(deviations) / len(deviations) if deviations else 0
                value *= age_deviation_multiplier

                results.append(KpiResult(
                    name=kpi.name,
                    scope="global",
                    value=round(value, 2),
                    description=kpi.description
                ))

            # HSEBAL — Housing Balance Variance
            elif kpi.metric == "housing_balance":
                # If no clear target yet, assume equal ideal distribution
                if total_birds > 0:
                    ideal_share = total_birds / max(1, len(housing_actual))
                    diffs = [(qty - ideal_share) for qty in housing_actual.values()]

                    value = variance(diffs) if len(diffs) > 1 else 0
                else:
                    value = 0

                value *= housing_balance_multiplier

                results.append(KpiResult(
                    name=kpi.name,
                    scope="global",
                    value=round(value, 2),
                    description=kpi.description
                ))

            # COST — Cost Estimate
            elif kpi.metric == "cost_estimate":
                value = total_allocated * cost_per_bird

                results.append(KpiResult(
                    name=kpi.name,
                    scope="global",
                    value=round(value, 2),
                    description=kpi.description
                ))

            # DISPIM — Disruption Impact %
            elif kpi.metric == "disruption_impact":
                affected = sum(p.allocated_qty for p in placements if p.reason_unplaced)

                value = (affected / total_birds * 100) if total_birds > 0 else 0
                value *= disruption_impact_multiplier

                results.append(KpiResult(
                    name=kpi.name,
                    scope="global",
                    value=round(value, 2),
                    description=kpi.description
                ))

        return results
