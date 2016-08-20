class PaginationHelper:
    # The constructor takes in an array of items and a integer indicating
    # how many items fit within a single page
    def __init__(self, collection, items_per_page):
        self._item_count = len(collection)
        self._items_per_page = items_per_page

    # returns the number of items within the entire collection
    def item_count(self):
        return self._item_count

    # returns the number of pages
    def page_count(self):
        return (self._item_count + self._items_per_page - 1) // self._items_per_page

    # returns the number of items on the current page. page_index is zero based
    # this method should return -1 for page_index values that are out of range
    def page_item_count(self, page_index):
        pages = self.page_count()
        if 0 <= page_index < pages - 1:
            return self._items_per_page
        elif page_index == pages - 1:
            return self._item_count % self._items_per_page
        else:
            return -1

    # determines what page an item is on. Zero based indexes.
    # this method should return -1 for item_index values that are out of range
    def page_index(self, item_index):
        return item_index // self._items_per_page if 0 <= item_index < self._item_count else -1


helper = PaginationHelper(range(0, 10), 3)

for page in range(0, helper.page_count() + 2):
    print("Page", page, " - ", helper.page_item_count(page), "items")

for item in range(0, helper.item_count() + 2):
    print("Item", item, "is on", helper.page_index(item), "page")
