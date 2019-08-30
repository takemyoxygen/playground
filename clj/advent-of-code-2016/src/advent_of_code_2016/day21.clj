(ns advent-of-code-2016.day21)

(defn to-int [x] (Integer/parseInt x))

(defn swap-position
  [x y input]
  (let [input-vec (vec input)]
    (map-indexed
      (fn [idx it]
        (case idx
          x (input-vec y)
          y (input-vec x)
          it))
      input)))

(defn swap-letter
  [x y input]
  (map
    (fn [i] (cond
              (= i x) y
              (= i y) x
              :else i))
    input))

(defn rotate-dir
  [direction steps input]
  (let [normalized-steps (rem steps (count input))
        pivot-point (case direction :left normalized-steps :right (- (count input) normalized-steps))
        [left right] (split-at pivot-point input)]
    (concat right left)))

(defn rotate-based
  [x input]
  (let [input-vec (vec input)
        index (.indexOf input-vec x)
        steps (+ (inc index) (if (>= index 4) 1 0))]
    (rotate-dir :right steps input)))

(defn reverse-pos
  [x y input]
  (let [[first interim] (split-at x input)
        [middle last] (split-at (inc (- y x)) interim)]
    (concat first (reverse middle) last)))

(defn move-pos
  [x y input]
  (let [input-vec (vec input)
        target (input-vec x)]
    (loop [[x & rest] input acc [] idx 0]
      )))

;(defn parse-command
;  [[type subtype & rest :as command]]
;  (case [type subtype]
;    ["swap" "position"] {:type :swap-position :x (to-int (command 2)) :y (to-int (command 5))}
;    ["swap" "letter"] {:type :swap-letter :x  :y (to-int (command 5))}
;    [type subtype]))

(defn parse-input
  [input]
  (->> input
       (clojure.string/split-lines)
       (map #(clojure.string/split % #"\s+"))))


(def input "rotate right 3 steps\nswap position 7 with position 0\nrotate left 3 steps\nreverse positions 2 through 5\nmove position 6 to position 3\nreverse positions 0 through 4\nswap position 4 with position 2\nrotate based on position of letter d\nrotate right 0 steps\nmove position 7 to position 5\nswap position 4 with position 5\nswap position 3 with position 5\nmove position 5 to position 3\nswap letter e with letter f\nswap position 6 with position 3\nswap letter a with letter e\nreverse positions 0 through 1\nreverse positions 0 through 4\nswap letter c with letter e\nreverse positions 1 through 7\nrotate right 1 step\nreverse positions 6 through 7\nmove position 7 to position 1\nmove position 4 to position 0\nmove position 4 to position 6\nmove position 6 to position 3\nswap position 1 with position 6\nswap position 5 with position 7\nswap position 2 with position 5\nswap position 6 with position 5\nswap position 2 with position 4\nreverse positions 2 through 6\nreverse positions 3 through 5\nmove position 3 to position 5\nreverse positions 1 through 5\nrotate left 1 step\nmove position 4 to position 5\nswap letter c with letter b\nswap position 2 with position 1\nreverse positions 3 through 4\nswap position 3 with position 4\nreverse positions 5 through 7\nswap letter b with letter d\nreverse positions 3 through 4\nswap letter c with letter h\nrotate based on position of letter b\nrotate based on position of letter e\nrotate right 3 steps\nrotate right 7 steps\nrotate left 2 steps\nmove position 6 to position 1\nreverse positions 1 through 3\nrotate based on position of letter b\nreverse positions 0 through 4\nswap letter g with letter c\nmove position 1 to position 5\nrotate right 4 steps\nrotate left 2 steps\nmove position 7 to position 2\nrotate based on position of letter c\nmove position 6 to position 1\nswap letter f with letter g\nrotate right 6 steps\nswap position 6 with position 2\nreverse positions 2 through 6\nswap position 3 with position 1\nrotate based on position of letter h\nreverse positions 2 through 5\nmove position 1 to position 3\nrotate right 1 step\nrotate right 7 steps\nmove position 6 to position 3\nrotate based on position of letter h\nswap letter d with letter h\nrotate left 0 steps\nmove position 1 to position 2\nswap letter a with letter g\nswap letter a with letter g\nswap position 4 with position 2\nrotate right 1 step\nrotate based on position of letter b\nswap position 7 with position 1\nrotate based on position of letter e\nmove position 1 to position 4\nmove position 6 to position 3\nrotate left 3 steps\nswap letter f with letter g\nswap position 3 with position 1\nswap position 4 with position 3\nswap letter f with letter c\nrotate left 3 steps\nrotate left 0 steps\nrotate right 3 steps\nswap letter d with letter e\nswap position 2 with position 7\nmove position 3 to position 6\nswap position 7 with position 1\nswap position 3 with position 6\nrotate left 5 steps\nswap position 2 with position 6")

(->> input
     (parse-input)
     (filter)
     (println))

;(println (swap-position 4 0 "abcde"))
;(println (swap-letter \d \b "ebcda"))
;(println (rotate-dir :left 1 "abcd"))
;(println (rotate-dir :right 1 "abcd"))

;(println (rotate-based \b "abdec"))
;(println (rotate-based \d "ecabd"))

(println (reverse-pos 1 4 "abcdef"))