import re

def select_group_targets(armyA, armyB):
    """Determine which opponent group each group within armyA should attack"""

    result = []
    for groupA in armyA:
        if groupA["units"] <= 0:
            continue
        targetpair = None
        for groupB in armyB:
            if groupB["target"] == True or groupB["units"] <= 0:
                continue
            if groupA["damageType"] in groupB["immuneTo"]:
                continue
            if groupA["damageType"] in groupB["weakTo"]:
                groupB["target"] = True
                result.append((groupA, groupB))
                targetpair = None
                break
            if targetpair == None:
                targetpair = (groupA, groupB)
        if targetpair != None:
            targetpair[1]["target"] = True
            result.append(targetpair)
    return result


def select_targets(immune, infection):
    """Select appropriate attack targets for each army"""

    immune.sort(key=lambda x: x["initiative"], reverse=True)
    immune.sort(key=lambda x: x["power"], reverse=True)
    infection.sort(key=lambda x: x["initiative"], reverse=True)
    infection.sort(key=lambda x: x["power"], reverse=True)

    result = []
    aa = select_group_targets(immune, infection)
    bb = select_group_targets(infection, immune)
    result.extend(aa)
    result.extend(bb)
    return result


def clear_target(army):
    """Reset the 'target' property for each group in this army"""

    for group in army:
        group["target"] = False


def attack(attacker, defender):
    """Perform the actual attack"""

    aunits = attacker["units"]
    if aunits <= 0:
        return

    damage = attacker["damage"]
    damagetype = attacker["damageType"]
    dunits = defender["units"]
    hp = defender["hp"]
    if damagetype in defender["weakTo"]:
        damage *= 2
    if damagetype in defender["immuneTo"]:
        damage = 0
    killed = (damage * aunits) // hp
    defender["units"] = (dunits - killed) if dunits >= killed else 0
    defender["power"] = defender["units"] * defender["damage"]


def is_defeated(army):
    """Determine if this army has any units left"""

    for group in army:
        if group["units"] > 0:
            return False
    return True

def unit_total(army):
    """Get the current total of all units in this army"""

    result = 0
    for group in army:
        result += group["units"]
    return result


def do_battle(armyA, armyB):
    """Fight for as many rounds as is necessary to find a winner or reach a stalemate"""

    while True:
        # Target selection
        clear_target(armyA)
        clear_target(armyB)
        targetlist = select_targets(armyA, armyB)

        if (len(targetlist) == 0):
            return (False, -1) # Nobody can attack - each one is immune to the other's damage

        # Attack
        targetlist.sort(key=lambda x: x[0]["initiative"], reverse=True)
        #for targetpair in targetlist:
        #    print(targetpair[0]["groupId"], " => ", targetpair[1]["groupId"])
        for targetpair in targetlist:
            attack(targetpair[0], targetpair[1])

        if is_defeated(armyA):
            return (False, unit_total(armyB))
        if is_defeated(armyB):
            return (True, unit_total(armyA))


def build_armies():
    """Read input file and create immune and infection group arrays"""

    rr = re.compile(r"(\d+) units each with (\d+) hit points (\((.*)\))?\s?with an attack that does (\d+) (\w+) damage at initiative (\d+)")

    f = open("./day24-input.txt", "r")
    #f = open("./day24-input-test.txt", "r") # Expect P1 == 5216; P2 == 51
    input = f.readlines()

    immunearmy, infectionarmy = [], []
    immune, infection = False, False
    immid, infid = 1, 1
    for line in [l.strip() for l in input]:
        if line.startswith("Immune System"):
            immune = True
            continue
        elif line.startswith("Infection"):
            immune = False
            infection = True
            continue
        elif line.isspace() or line == "":
            continue

        match = rr.split(line)
        immuneto = []
        weakto = []
        if match[4] != None:
            parts = [p.strip() for p in match[4].strip().split(';')]
            for part in parts:
                if part.startswith("weak to"):
                    ww = [p.strip() for p in part[8:].strip().split(',')]
                    weakto.extend(ww)
                else:
                    ii = [p.strip() for p in part[10:].strip().split(',')]
                    immuneto.extend(ii)
        group = {
            "groupType": "immune" if immune else "infection",
            "groupId": immid if immune else infid,
            "units": int(match[1]),
            "hp": int(match[2]),
            "damage": int(match[5]),
            "damageType": match[6],
            "initiative": int(match[7]),
            "conditions": match[4],
            "weakTo": weakto,
            "immuneTo": immuneto,
            "power": int(match[1]) * int(match[5]),
            "target": False,
        }
        if immune:
            immunearmy.append(group)
            immid += 1
        elif infection:
            infectionarmy.append(group)
            infid += 1

    return (immunearmy, infectionarmy)


def main():
    immunearmy, infectionarmy = build_armies()

    # Use proper/deep copies of the armies
    imm = [dict(g) for g in immunearmy]
    inf = [dict(g) for g in infectionarmy]
    part1 = do_battle(imm, inf)
    print("Part 1: Winner has", part1[1], "units left")

    part2 = (False, 0)
    boost = 0
    while part2[0] == False:
        boost += 1
        # Get new copies of each army
        army1 = [dict(g) for g in immunearmy]
        army2 = [dict(g) for g in infectionarmy]
        # Update damage and power by the boost amount
        for group in army1:
            group["damage"] += boost
            group["power"] = group["units"] * group["damage"]

        part2 = do_battle(army1, army2)

    print("Part 2: Immune system wins with a boost of", boost, "and has", part2[1], "units left")


if __name__ == "__main__":
    main()
