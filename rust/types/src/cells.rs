use std::rc::Rc;
use std::cell::Cell;
use std::cell::{RefCell, RefMut};

// #[derive(Sized)]
struct Person {
    age: i32,
    name: String
}

fn own(p: Rc<Person>) {
    print!("Name: {}, age: {}", p.name, p.age);
}

pub fn persons() -> Option<()> {
    let p = Person {age: 44, name: "John Doe".to_string()};;
    let p = Rc::new(p);
    own(p.clone());
    let weak = Rc::downgrade(&p);

    drop(p);

    weak.upgrade().map(|_| ())
}

pub struct Container {
    content: RefCell<Option<String>>
}

impl Container {
    pub fn try_init(&self, content: String) {
        let mut current = self.content.borrow_mut();
        if current.is_none() {
            *current = Some(content);
        }
    }
}

pub struct RefCounted<T: Clone> {
    value: T,
    refcount: Cell<usize>
}

impl<T: Clone> RefCounted<T> {
    pub fn claim(&self) -> T {
        let value = self.refcount.get();
        self.refcount.set(value + 1);

        self.value.clone()
    }

    pub fn new(value: T) -> RefCounted<T> {
        RefCounted {value: value, refcount: Cell::new(0)}
    }
}

pub fn get_modified_value() -> i32{
    let x = Cell::new(1);
    let y = &x;
    let z = &x;
    x.set(2);
    y.set(3);
    z.set(4);
    x.get()
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::cell::RefCell;

    #[test]
    fn dropped_refs() {
        assert_eq!(persons(), None);
    }

    #[test]
    fn try_init_test(){
        let c = Container{content: RefCell::new(None)};
        let expected = "foo".to_string();

        c.try_init(expected.clone());

        let content = c.content.borrow();
        assert_eq!(Some(expected.clone()), *content);
    }

    #[test]
    fn ref_count_tests() {
        let expected = "foo";
        let x = RefCounted::new(expected);
        assert_eq!(0, x.refcount.get());

        let claimed = x.claim();
        assert_eq!(expected, claimed);
        assert_eq!(1, x.refcount.get());
    }

    #[test]
    fn get_modified_value_test(){
        assert_eq!(4, get_modified_value());
    }
}
