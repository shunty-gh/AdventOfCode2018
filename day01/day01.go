package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

func main() {
	file, err := os.Open(".\\day01-part1.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	// Get a list of integer values
	var input []int64
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		val, _ := strconv.ParseInt(scanner.Text(), 0, 64)
		input = append(input, val)
	}

	seen := make(map[int64]int)
	current := int64(0)
	part1 := int64(0)
	part2 := int64(0)
	found := false
	iter := 0

	for found == false {
		iter++
		for i := range input {
			current += input[i]

			// Check if we've seen this frequency before
			if _, ok := seen[current]; ok {
				// Found it
				part2 = current
				found = true
				if iter > 1 {
					break
				}
			} else {
				seen[current] = 1
			}
		}
		if iter == 1 {
			part1 = current
		}
	}

	fmt.Println("Part 1", part1)
	fmt.Println("Part 2", part2)
}
