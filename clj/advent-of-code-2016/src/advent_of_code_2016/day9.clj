(ns advent-of-code-2016.day9)

(defn read-until
  "Reads symbols from incoming string until it encounters a given stop-at symbol. Returns
  a pair - symbols read before stop-at symbol and remaining string (starting with stop-at symbol)"
  [input stop-at]
  (loop [[x & rest :as current-input] input acc '()]
    (if
      (or (empty? current-input) (= x stop-at)) [(apply str (reverse acc)) current-input]
      (recur rest (cons x acc)))))

(defn read-repeat
  "Expects a string starting from repetition marker e.g. \"(3x10)ABC\", extracts repetition params
  and returns the rest of the input string e.g. [3 10 \"ABC\"]"
  [[_ & rest]]
  (let [[repeat-length after-repeat-count] (read-until rest \x)
        [repeat-times after-repeat-times] (read-until (next after-repeat-count) \))]
    [(Integer/parseInt repeat-length) (Integer/parseInt repeat-times) (next after-repeat-times)]))

(defn process-repeat
  [input]
  (let [[repeat-length repeat-times rest] (read-repeat input)
        repetition-length (* repeat-times repeat-length)
        rest-after-repetition (drop repeat-length rest)]
    [repetition-length rest-after-repetition]))

(defn sanitize
  "Prepares input (removes whitespaces) to be passed further to calculation functions"
  [input]
  (clojure.string/replace input #"\s+" ""))

(defn calculate-decompressed-size
  "Decompresses repeatitions encountered in the given input and returns decompressed input length"
  [input]
  (loop [current-length 0 [c & rest :as to-process] (char-array input)]
    (cond
      (nil? c) current-length
      (= c \() (let [[repeated-length rest-after-repetition] (process-repeat to-process)]
                 (recur (+ current-length repeated-length) rest-after-repetition))
      :else (recur (inc current-length) rest))))


(defn solve
  [input]
  (let [sanitized-input (sanitize input)]
    (calculate-decompressed-size sanitized-input)))

(defn solve-part1
  [input]
  (let [length (solve input)]
    (println "Converted length" length)))
