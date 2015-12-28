pub fn fact(n: i32) -> i32 {
    match n {
        0 => 1,
        _ => n * fact(n - 1)
    }
}

#[cfg(test)]
mod tests {
    use super::fact;

    #[test]
    fn fact_3(){
        assert_eq!(6, fact(3));
    }

    fn fact_6(){
        assert_eq!(720, fact(6));
    }
}
