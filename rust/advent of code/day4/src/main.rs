extern crate md5;

use std::io;
use std::io::Read;
use md5::Digest;

fn read_input() -> io::Result<String> {
    let mut buffer = String::new();
    try!(io::stdin().read_to_string(&mut buffer));
    Ok(buffer.trim().to_string())
}

fn starts_with_five_zeros(digest: &Digest) -> bool {
    digest[0] == 0 && digest[1] == 0 && digest[2] < 16
}

fn sixth_item_is_also_zero(digest: &Digest) -> bool {
    digest[2] == 0
}

fn main() {
    let input = read_input().unwrap();

    let _: Vec<_> = (0..i64::max_value())
        .map(|i| (i, input.clone() + &i.to_string()))
        .map(|(i, s)| (i, md5::compute(s.as_bytes())))
        .filter(|&(_, digest)| starts_with_five_zeros(&digest))
        .inspect(|&(i, _)| println!("With 5 zeros: {}", i))
        .skip_while(|&(_, digest)| !sixth_item_is_also_zero(&digest))
        .inspect(|&(i, _)| println!("With 6 zeros: {}", i))
        .take(1)
        .collect();
}
