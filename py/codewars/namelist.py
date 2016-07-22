from functools import *


def namelist(names):
    return reduce(
        lambda acc, n: acc + (", " if 0 < n[0] < len(names) - 1 else "" if n[0] == 0 else " & ") + n[1],
        enumerate(map(lambda d: d["name"], names)),
        "")

def nameslist2(names):
    names = list(map(lambda d: d["name"], names))
    return ", ".join(names[:-1]) + ""

print(namelist([ {'name': 'Bart'}, {'name': 'Lisa'}, {'name': 'Maggie'} ]))
# returns 'Bart, Lisa & Maggie'

print(namelist([ {'name': 'Bart'}, {'name': 'Lisa'} ]))
# returns 'Bart & Lisa'

print(namelist([ {'name': 'Bart'} ]))
# returns 'Bart'

print(namelist([]))
# returns ''

