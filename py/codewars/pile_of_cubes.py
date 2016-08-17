import itertools

def find_nb(m):
    sum = 0
    for i in itertools.count(1):
        sum += i ** 3
        if sum == m:
            return i
        elif sum > m:
            return -1