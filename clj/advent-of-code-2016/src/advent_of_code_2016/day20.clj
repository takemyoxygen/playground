(ns advent-of-code-2016.day20)

(defn parse-input
  [input]
  (->> input
       (clojure.string/split-lines)
       (map #(re-matches #"(\d+)-(\d+)" %))
       (map (fn [[_ lo hi]] [(bigint lo) (bigint hi)]))))

(def max-ip 4294967295)

(defn step
  [[acc last-hi] [lo hi]]
  (let [next-acc (if (> (- lo last-hi) 1) (cons [(inc last-hi) (dec lo)] acc) acc)]
    [next-acc (max hi last-hi)]))

(defn get-open-ranges
  [initial-blacklist]
  (let [[open-ranges last-hi] (reduce step [[] 0] (sort-by first initial-blacklist))
        open-range-with-last (if (= last-hi max-ip) open-ranges (cons [(inc last-hi) max-ip] open-ranges))]
    (reverse open-range-with-last)))

(defn solve-part-1
  [input]
  (->> input
      (parse-input)
      (get-open-ranges)
      (first)
      (first)))

(defn solve-part-2
  [input]
  (->> input
       (parse-input)
       (get-open-ranges)
       (map (fn [[lo hi]] (inc (- hi lo))))
       (reduce +)))