(ns advent-of-code-2016.day24)

(defn parse-input
  [input]
  (->> input
       (clojure.string/split-lines)
       (map-indexed
         (fn [row-number row]
           (map-indexed (fn [column-number symbol] [[column-number row-number] symbol]) row)))
       (apply concat)
       (into (sorted-map))))

(defn find-start-location
  [layout]
  (->> layout
       (filter (fn [[_ val]] (= val \0)))
       (map first)
       (first)))

(defn get-available-adjacent-locations
  [layout [x y]]
  (for [neighbor [[(inc x) y] [(dec x) y] [x (inc y)] [x (dec y)]]
        :let [neighbor-val (get layout neighbor)]
        :when (and (not (nil? neighbor-val)) (not= neighbor-val \#))]
    neighbor))

(defn req-loc? [val] (and (not= val \.) (not= val \#)))

(defn get-required-locations-count
  [layout]
  (->> layout
       (filter (fn [[_ val]] (req-loc? val)))
       (count)))

(defn step
  [layout visited-req-locations current-path-length shortest-path-length-so-far current-location req-locations-count prev-loc]
  (println "Visiting" current-location)
  (cond
    (= (count visited-req-locations) req-locations-count) current-path-length
    (= shortest-path-length-so-far current-path-length) shortest-path-length-so-far
    :else
      (let [next-locations (get-available-adjacent-locations layout current-location)
            curr-value (get layout current-location)
            next-req-locations (if (req-loc? curr-value) (conj visited-req-locations curr-value) visited-req-locations)]
        (println "Got" (dec (count next-locations)) "locations next")
        (reduce
          (fn [shortest-so-far loc]
            (step layout next-req-locations (inc current-path-length) shortest-so-far loc req-locations-count current-location))
          shortest-path-length-so-far
          (filter #(not= prev-loc %) next-locations)))))

(def sample-input "###########\n#0.1.....2#\n#.#######.#\n#4.......3#\n###########")
(def sample-layout (parse-input sample-input))
(def start (find-start-location sample-layout))



(get-available-adjacent-locations sample-layout [5 1])

(step
  sample-layout
  #{}
  0
  12333
  start
  (get-required-locations-count sample-layout)
  nil)