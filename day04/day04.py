# AoC 2018 day 4

from collections import defaultdict

f = open("./day04-part1.txt", "r")
input = f.readlines()
f.close()

guard = -1
sleepstart = -1
sleeping = defaultdict(lambda: defaultdict(int))
for _, line in enumerate(sorted(input)):
    l = line.strip()
    minute = int(line[15:17])
    if l.endswith("shift"):
        guard = int(l[26:30])
    elif l.endswith("asleep"):
        sleepstart = minute
    else: # wakes up
        for m in range(sleepstart, minute):
            sleeping[guard][m] += 1

# Part 1 - sum all the minutes asleep for each guard and get the max
d1 = {k:sum(v.values()) for k,v in sleeping.items()}
g1 = max(d1, key=d1.get)
v1 = max(sleeping[g1], key=sleeping[g1].get)
print("Part 1:", g1 * v1)

# Part 2 - find the sleepiest minute for each guard and get the largest of all
d2 = {k:max(v.values()) for k,v in sleeping.items()}
g2 = max(d2, key=d2.get)
v2 = max(sleeping[g2], key=sleeping[g2].get)
print("Part 2:", g2 * v2)
