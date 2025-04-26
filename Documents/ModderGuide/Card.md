# ✨ Adding a New Card

This guide explains how to add new cards to your project,
by defining their logic, properties, and localization.

## 1. Add Card YAML Definition

Create a YAML file under your cards/ directory.

Each card consists of:
- Basic attributes (cost, rarity, target, tags)
- Properties (such as attack, block)
- Actions triggered when the card is played (onplay)

### Example: "Iron Strike"

> Gain 5 Block. Deal 5 damage.

```yaml
id: Custom::IronStrike
cost: 1
rarity: Common
target: EnemySingle
tags:
  - strike
properties:
  - attack: 5
  - block: 5
onplay:
  - actionid: DBRE::GainBlock
    bindings:
      block:
        propertyid: block
  - actionid: DBRE::DealDamageToTarget
    animation: DBRE::SlashHorizontal
    bindings:
      attack:
        propertyid: attack
```

### Example: "Pummel"

> Deal 7 damage X times.

```yaml
id: Custom::Pummel
cost: X
rarity: Uncommon
target: EnemySingle
properties:
  - attack: 7
onplay:
  - action: DBRE::DealDamage
    animation: DBRE::SlashHeavy
    bindings:
      attack:
        propertyid: attack
      count:
        contextid: cost_paid
```

## 2. Add Localization Strings

To display card text, add localization entries:

```yaml
localizations:
  - id: Custom::IronStrike
    text: Gain {block} Block. Deal {attack} damage.
  - id: Custom::Pummel
    text: Deal {attack} damage X times.
```

## 3. (Optional) Adjust Card Stats via CSV

You can override card definitions and properties using a `cardstats.csv` file.

This simplifies YAML files and enables easy mass-balance adjustments.

### Example: `cardstats.csv`

|id|cost|rarity|target|attack|block|
|---|---|---|---|---|---|
|Custom::IronWave|1|Common|EnemySingle|5|5|
|Custom::Eviscerate|X|Uncommon|EnemySingle|7| |

### Simplified `Pummel.yaml` with CSV

```yaml
id: Custom::Pummel
onplay:
  - action: DBRE::DealDamage
    animation: DBRE::SlashHeavy
    bindings:
      attack:
        propertyid: attack
      count:
        contextid: cost_paid
```

## Notes

- Always use unique id for each card.
- Use actionid to refer to pre-defined actions.
- animationid is optional and specifies the visual effect.
- contextid bindings allow using runtime variables (e.g., cost_paid).

✅ Now you're ready to easily add new cards,
with clean separation of logic, data, and visuals!