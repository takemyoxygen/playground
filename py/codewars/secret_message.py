from collections import OrderedDict


def find_secret_message(paragraph):
    words = [''.join(filter(lambda c: c.isalpha() or c == '-', word.lower())) for word in paragraph.split()]
    seen = set()
    duplicates = OrderedDict()
    for word in words:
        if word not in seen:
            seen.add(word)
        else:
            duplicates[word] = None

    return ' '.join(duplicates.keys())
