package main

import (
	"fmt"
)

type caveLocation struct {
	X, Y int
}

type caveState struct {
	CaveType, ErosionLevel int
}

type visitState struct {
	X, Y, Tool, Time int
}

type visitedState struct {
	X, Y, Tool int
}

const TargetX = 13
const TargetY = 726
const Depth = 3066

// Test values - P1 => 114; P2 => 45
// const TargetX = 10
// const TargetY = 10
// const Depth = 510

// Cave type
const Rocky = 0
const Wet = 1
const Narrow = 2

// Tools
const None = 0
const Torch = 1
const ClimbingGear = 2

func main() {
	var p1 = 0
	el := 0
	cave := make(map[caveLocation]caveState)
	for y := 0; y <= TargetY; y++ {
		for x := 0; x <= TargetX; x++ {
			el = erosionLevel(geoIndex(cave, x, y), Depth)
			cave[caveLocation{x, y}] = caveState{el % 3, el}
			p1 += (el % 3)
		}
	}

	fmt.Println("Part 1: ", p1)
	fmt.Println("Part 2: ", part2(cave))
}

func part2(cave map[caveLocation]caveState) int {
	visited := make(map[visitedState]int)
	var current visitState
	bestTime := ((TargetX + TargetY) * 7) + 1 // Manhattan distance with a tool change every time
	tovisit := []visitState{{0, 0, Torch, 0}}
	moves := []caveLocation{{1, 0}, {0, -1}, {-1, 0}, {0, 1}}
	for len(tovisit) > 0 {
		// Pop current
		current = tovisit[0]
		tovisit = tovisit[1:]

		if current.Time >= bestTime-1 { //no point in continuing if it's already slower
			continue
		}

		// Check if we've already visited, with this tool AND when we did so
		// our total time was less than it is now
		if vs, ok := visited[visitedState{current.X, current.Y, current.Tool}]; ok && vs <= current.Time {
			continue
		}
		visited[visitedState{current.X, current.Y, current.Tool}] = current.Time

		// Visit neighbours
		for _, dxy := range moves {
			nextX, nextY := current.X+dxy.X, current.Y+dxy.Y
			// Is it a valid location
			if nextX < 0 || nextY < 0 {
				continue
			}
			// Are we there yet
			if nextX == TargetX && nextY == TargetY {
				timeTaken := current.Time + 1
				if current.Tool != Torch {
					timeTaken += 7
				}
				if timeTaken < bestTime {
					bestTime = timeTaken
					//fmt.Println("Best time reduced to: ", bestTime)
				}
				continue
			}

			// Get state of next region - check it exists first
			nextLoc := caveLocation{nextX, nextY}
			if _, ok := cave[nextLoc]; !ok {
				gi := geoIndex(cave, nextX, nextY)
				el := erosionLevel(gi, Depth)
				cave[nextLoc] = caveState{el % 3, el}
			}

			thisType := cave[caveLocation{current.X, current.Y}].CaveType
			nextType := cave[nextLoc].CaveType
			nextTool := current.Tool
			nextTime := current.Time + 1
			switch nextType {
			case Rocky: // Can only visit if current tool is climbing gear or torch
				if nextTool != Torch && nextTool != ClimbingGear {
					nextTime += 7
					if thisType == Wet {
						nextTool = ClimbingGear
					} else { // Must be Narrow region - change to torch
						nextTool = Torch
					}
				}
			case Wet: // Can only visit if current tool is climbing gear or neither
				if nextTool != None && nextTool != ClimbingGear {
					nextTime += 7
					if thisType == Rocky {
						nextTool = ClimbingGear
					} else { // Must be Narrow region - change to neither
						nextTool = None
					}
				}
			case Narrow: // Can only visit if current tool is torch or neither
				if nextTool != None && nextTool != Torch {
					nextTime += 7
					if thisType == Rocky {
						nextTool = Torch
					} else { // Must be Wet region - change to neither
						nextTool = None
					}
				}
			}
			if vs, ok := visited[visitedState{nextX, nextY, nextTool}]; (!ok || vs > nextTime) && nextTime < bestTime-1 {
				tovisit = append(tovisit, visitState{nextX, nextY, nextTool, nextTime})
			}
		}
	}
	return bestTime
}

func geoIndex(cave map[caveLocation]caveState, x, y int) int {
	if (x == 0 && y == 0) || (x == TargetX && y == TargetY) {
		return 0
	} else if y == 0 {
		return x * 16807
	} else if x == 0 {
		return y * 48271
	} else {
		return cave[caveLocation{x - 1, y}].ErosionLevel * cave[caveLocation{x, y - 1}].ErosionLevel
	}
}

func erosionLevel(geoIndex int, depth int) int {
	return (geoIndex + depth) % 20183
}
