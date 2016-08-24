import numpy as np


def determinant(matrix):
    return np.linalg.det(np.array(matrix))

m = [[2, 5, 3], [1, -2, -1], [1, 3, 4]]


print(determinant([[5]]))