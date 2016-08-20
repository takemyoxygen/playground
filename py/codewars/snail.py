from pprint import pprint


def snail(array):

    rotate = lambda a: list(zip(*a))[::-1]
    return list(array[0]) + snail(rotate(array[1:])) if array else []

    #
    # pprint(array)
    # directions = [(0, 1), (1, 0), (0, -1), (-1, 0)]
    #
    # def _iter(start, direction_index, n, left):
    #     current = start
    #     direction = directions[direction_index]
    #
    #     for _ in range(0, n):
    #         current = (current[0] + direction[0], current[1] + direction[1])
    #         yield array[current[0]][current[1]]
    #
    #     n, left = (n, left - 1) if left > 1 else (n - 1, 2)
    #
    #     if n > 0:
    #         yield from _iter(current, (direction_index + 1) % len(directions), n, left)
    #
    # return list(_iter((0, -1), 0, 0 if len(array) == 0 else len(array[0]), 1))


array = [[1,2,3, 1],
         [4,5,6, 4],
         [7,8,9, 7],
         [7,8,9,7]]

print(snail(array))

array = [[1, 2, 3], [4, 5, 6], [7, 8, 9]]
def rotate(array):
    return list(zip(*array))[::-1]

boo