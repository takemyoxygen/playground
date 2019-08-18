(ns advent-of-code-2016.day17
  (:require [advent-of-code-2016.common :as common])
  (:import (clojure.lang PersistentQueue)))

(def open-door-states #{\b \c \d \e \f})

; up, down, left, right
(def move-offsets [[0 -1] [0 1] [-1 0] [1 0]])

(def open? (partial contains? open-door-states))

(defn in-bounds?
  [[x y]]
  (and (>= 3 x 0) (>= 3 y 0)))

(defn next-doors
  [input path]
  (->> (str input path)
       (common/md5)
       (take 4)
       (map open?)))

(defn possible-moves
  [current-position input current-path]
  (->> (next-doors input current-path)
       (map vector move-offsets)
       (filter second)
       (map first)
       (map #(map + % current-position))
       (filter in-bounds?)))

(defn find-path
  [start destination input]
  (loop [queue (conj PersistentQueue/EMPTY [start ""])]
    (if (zero? (rem (count queue) 1000)) (println "Queue size:" (count queue)))
    (if-let [[current-pos current-path] (peek queue)]
      (if (= current-pos destination)
          current-path
          (recur (apply conj (pop queue) (possible-moves current-pos input current-path)))))))