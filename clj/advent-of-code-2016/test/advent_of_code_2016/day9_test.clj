(ns advent-of-code-2016.day9-test
  (:require [clojure.test :refer :all]
            advent-of-code-2016.day9))

(deftest part1
  (testing "no markers"
    (is (= (advent-of-code-2016.day9/solve "ADVENT") (count "ADVENT"))))

  (testing "single marker"
    (is (= (advent-of-code-2016.day9/solve "A(1x5)BC") (count "ABBBBBC"))))

  (testing "single marker at start"
    (is (= (advent-of-code-2016.day9/solve "(3x3)XYZ") (count "XYZXYZXYZ"))))

  (testing "several markers"
    (is (= (advent-of-code-2016.day9/solve "A(2x2)BCD(2x2)EFG") (count "ABCBCDEFEFG"))))

  (testing "markers in repeated part"
    (is (= (advent-of-code-2016.day9/solve "X(8x2)(3x3)ABCY") (count "X(3x3)ABC(3x3)ABCY"))))

  )

