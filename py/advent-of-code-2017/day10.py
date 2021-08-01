from typing import List, Iterable
from functools import reduce
from operator import xor


class Circle:
    def __init__(self, values: Iterable[int]):
        self.__values = list(values)

    def normalize_index(self, index: int) -> int:
        return index % len(self.__values)

    def swap(self, i: int, j: int) -> None:
        i = self.normalize_index(i)
        j = self.normalize_index(j)
        self.__values[i], self.__values[j] = self.__values[j], self.__values[i]

    def __getitem__(self, item: int) -> int:
        return self.__values[self.normalize_index(item)]

    def __str__(self):
        return str(self.__values)

    def values(self) -> List[int]:
        return self.__values.copy()


def reverse(circle: Circle, start: int, length: int) -> None:
    end = circle.normalize_index(start + length - 1)
    for i in range(length // 2):
        circle.swap(start + i, end - i)


def hash_round(circle: Circle, lengths: List[int], position: int = 0, skip: int = 0) -> tuple[int, int]:
    for length in lengths:
        reverse(circle, position, length)
        position = circle.normalize_index(position + length + skip)
        skip += 1
    return position, skip


def part1(circle: Circle, lengths: List[int]) -> int:
    hash_round(circle, lengths)
    return circle[0] * circle[1]


def hash_blocks(values: List[int]) -> List[int]:
    result = [0] * 16

    for i in range(16):
        start, end = i * 16, (i + 1) * 16
        result[i] = reduce(xor, values[start:end])

    return result


def hexify(x: int) -> str:
    hexed = hex(x)[2:]
    return hexed if len(hexed) == 2 else '0' + hexed


def part2(circle: Circle, lengths_string: str):
    lengths = [ord(c) for c in lengths_string] + [17, 31, 73, 47, 23]
    position, skip = 0, 0
    for _ in range(64):
        position, skip = hash_round(circle, lengths, position, skip)

    hashed_blocks = hash_blocks(circle.values())
    return ''.join(map(hexify, hashed_blocks))


print(part1(Circle(range(256)), [129, 154, 49, 198, 200, 133, 97, 254, 41, 6, 2, 1, 255, 0, 191, 108]))
print(part2(Circle(range(256)), '129,154,49,198,200,133,97,254,41,6,2,1,255,0,191,108'))
