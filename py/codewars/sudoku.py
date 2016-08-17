from itertools import chain


def triplet(xs, number):
    return xs[number * 3:(number + 1) * 3]


def regions(board):
    for row in range(0, 3):
        for col in range(0, 3):
            yield chain.from_iterable((triplet(r, col) for r in triplet(board, row)))


def columns(board):
    column = lambda col: (row[col] for row in board)
    return (column(col) for col in range(0, 9))


def done_or_not(board):
    expected = set(range(1, 10))
    dimensions = chain(board, columns(board), regions(board))
    matching = all(set(dim) == expected for dim in dimensions)
    return "Finished!" if matching else "Try again!"
