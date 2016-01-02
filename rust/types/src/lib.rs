mod cells;

use std::f32;
use std::num;

#[derive(Debug)]
pub enum Error {
    Input(String),
    Parsing(num::ParseIntError)
}

impl PartialEq for Error {
    fn eq(&self, another: &Self) -> bool {
        match (self, another){
            (&Error::Input(ref s1), &Error::Input(ref s2)) => s1 == s2,
            (&Error::Parsing(ref err1), &Error::Parsing(ref err2)) => err1 == err2,
            _ => false
        }
    }
}

impl Eq for Error {}

pub fn find_root(x: i32) -> Option<i32> {
    let y = (x as f32).sqrt();
    if y.fract() <= f32::EPSILON {
        Some(y as i32)
    } else {
        None
    }
}

// already exists as "and_then" function
pub fn bind<T, K, F>(opt: Option<T>, f: F) -> Option<K> where F: FnOnce(T) -> Option<K> {
    match opt {
        Some(x) => f(x),
        None => None
    }
}

pub fn as_string<T>(opt: Option<T>) -> String where T: ToString {
    match opt {
        Some(x) => x.to_string(),
        None => "None".to_string()
    }
}

pub fn parse_and_double(input: &str) -> Result<String, Error> {
    input
        .trim()
        .parse::<i32>()
        .map_err(|e| Error::Parsing(e))
        .map(|x| x * 2)
        .map(|x| x.to_string())
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn find_root_tests(){
        assert_eq!(Some(2), find_root(4));
        assert_eq!(Some(4), find_root(16));
        assert_eq!(None, find_root(5));
    }

    #[test]
    fn as_string_tests(){
        assert_eq!("5", as_string(Some(5)));
        assert_eq!("hey", as_string(Some("hey")));
        assert_eq!("None", as_string::<i32>(None));
    }

    #[test]
    fn bind_test(){
        assert_eq!(Some(5), bind(Some(4), |x| Some(x + 1)));
        assert_eq!(None::<i32>, bind(None::<i32>, |x| Some(x + 1)));
        assert_eq!(None, bind(Some("foo"), |_| (None::<i32>)));
    }

    #[test]
    fn parse_and_double_tests() {
        assert_eq!(Ok::<String, Error>("10".to_string()), parse_and_double("5"));
        assert_eq!(Ok::<String, Error>("-4".to_string()), parse_and_double("-2"));
        assert!(parse_and_double("foo").is_err());
    }

    #[test]
    fn error_equality_tests() {
        assert_eq!(Error::Input("foo".to_string()), Error::Input("foo".to_string()));

        let error1 = "foo".parse::<i32>().err().unwrap();
        let error2 = "foo".parse::<i32>().err().unwrap();
        assert_eq!(Error::Parsing(error1), Error::Parsing(error2));
    }
}
