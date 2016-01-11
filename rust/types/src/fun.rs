fn convert_to_string(x: i32) -> String {
    x.to_string()
}

pub fn five_as_string() -> String {
    let x = convert_to_string;
    x(5)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn five_as_string_tests(){
        assert_eq!("5".to_string(), five_as_string());
    }
}
