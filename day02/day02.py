# AoC 2018 day 2, parts 1 and 2

# Need to run
# $> pip install pyperclip
# if we want to copy the part 2 result to the clipboard
import pyperclip

f = open("./day02-part1.txt", "r")
input = f.readlines()
f.close()

# Part 1
twos = 0
threes = 0
for _, id in enumerate(input):
    counts = dict()
    for _, ch in enumerate(id):
        counts[ch] = counts.get(ch, 0) + 1

    if any(v == 2 for k, v in counts.items()): twos += 1
    if any(v == 3 for k, v in counts.items()): threes += 1

print("Part 1:", twos * threes)

# Part 2
found = False
for idx, id in enumerate(input):
    if found: break

    for _, id2 in enumerate(input[idx:]):
        diffs = 0
        diffindex = 0
        diffletter = ''
        for index, ch in enumerate(id):
            if id[index] != id2[index]:
                diffs += 1
                diffindex = index
                diffletter = id[index]
            if diffs > 1:
                break

        if diffs == 1:
            found = True
            print("Found it :", id.strip() + ";", "Letter", diffletter, "at index", diffindex)
            print("Part 2   :", id[:diffindex] + id[diffindex + 1:])
            pyperclip.copy(id[:diffindex] + id[diffindex + 1:])
            break
