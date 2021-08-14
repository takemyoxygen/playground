from typing import List
import itertools


def parse_input(lines: List[str]) -> dict[int, int]:
    layers = {}
    for line in lines:
        [layer, depth] = line.split(': ')
        layers[int(layer)] = int(depth)
    return layers


def get_scanner_position(time: int, depth: int) -> int:
    period = 2 * depth - 2
    period_pos = time % period
    return period_pos if period_pos < depth else period - period_pos


def calculate_severity(layers: dict[int, int], delay: int) -> tuple[int, int]:
    severity = 0
    times_caught = 0
    for layer in layers:
        depth = layers[layer]
        scanner_position = get_scanner_position(layer + delay, depth)
        if scanner_position == 0:
            severity += layer * depth
            times_caught += 1
    return severity, times_caught


def part1(layers: dict[int, int]) -> int:
    return calculate_severity(layers, 0)[0]


def part2(layers: dict[int, int]) -> int:
    delays = itertools.count()
    return next(delay for delay in delays if calculate_severity(layers, delay)[1] == 0)


lines = open('input/day13.txt').readlines()
layers = parse_input(lines)

print('Part 1:', part1(layers))
print('Part 2:', part2(layers))