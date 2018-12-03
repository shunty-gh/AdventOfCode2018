# AoC 2018 day 3, parts 1 and 2

import re

f = open("./day03-part1.txt", "r")
input = f.readlines()
f.close()

# Part 1
pattern = r"#(?P<id>\d*)\s@\s(?P<left>\d*),(?P<top>\d*):\s(?P<width>\d*)x(?P<height>\d*)"
claims = dict()
map = dict()
for _, claim in enumerate(input):
    m = re.match(pattern, claim)

    id = m.group("id")
    l = int(m.group("left"))
    t = int(m.group("top"))
    w = int(m.group("width"))
    h = int(m.group("height"))

    claims[id] = {'left': l, 'top': t, 'width': w, 'height': h}
    for i in range(w):
        for j in range(h):
            key = (l + i, t + j)
            map[key] = map.get(key, 0) + 1

overlaps = len({k:v for k, v in map.items() if v > 1})
# OR overlaps = len(dict(filter(lambda x: x[1] > 1, map.items())))
print("Part 1:", overlaps)

# Part 2
for k,v in claims.items():
    overlap = False

    l,t,w,h = v["left"], v["top"], v["width"], v["height"]
    for i in range(w):
        if overlap: break

        for j in range(h):
            key = (l + i, t + j)
            if (map[key] != 1):
                overlap = True
                break

    if (not overlap):
        print("Part 2:", k)
        break
