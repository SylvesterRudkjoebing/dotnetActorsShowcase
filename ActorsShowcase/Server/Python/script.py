import csv
import sys

# Global data structures
names = {}
people = {}
movies = {}

class Node:
    """ Node class for BFS """
    def __init__(self, state, parent, action):
        self.state = state
        self.parent = parent
        self.action = action

class StackFrontier:
    """ Stack Frontier Class for DFS """
    def __init__(self):
        self.frontier = []
    def add(self, node):
        self.frontier.append(node)
    def contains_state(self, state):
        return any(node.state == state for node in self.frontier)
    def empty(self):
        return len(self.frontier) == 0
    def remove(self):
        if self.empty():
            raise Exception("empty frontier")
        node = self.frontier[-1]
        self.frontier = self.frontier[:-1]
        return node

class QueueFrontier(StackFrontier):
    """ Queue Frontier Class for BFS """
    def remove(self):
        if self.empty():
            raise Exception("empty frontier")
        node = self.frontier[0]
        self.frontier = self.frontier[1:]
        return node

def load_data(directory):
    """ Load data from CSV files into memory. """
    # Load people
    with open(f"{directory}/people.csv", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            people[row["id"]] = {
                "name": row["name"],
                "birth": row["birth"],
                "movies": set()
            }
            if row["name"].lower() not in names:
                names[row["name"].lower()] = {row["id"]}
            else:
                names[row["name"].lower()].add(row["id"])

    # Load movies
    with open(f"{directory}/movies.csv", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            movies[row["id"]] = {
                "title": row["title"],
                "year": row["year"],
                "stars": set()
            }

    # Load stars
    with open(f"{directory}/stars.csv", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            try:
                people[row["person_id"]]["movies"].add(row["movie_id"])
                movies[row["movie_id"]]["stars"].add(row["person_id"])
            except KeyError:
                pass

def shortest_path(source, target):
    """ Find the shortest path from source to target using BFS. """
    start = Node(source, None, None)
    frontier = QueueFrontier()
    frontier.add(start)
    explored = set()

    while True:
        if frontier.empty():
            return None

        curr_node = frontier.remove()
        explored.add(curr_node.state)

        for action, state in neighbors_for_person(curr_node.state):
            if state == target:
                path = []
                path.append((action, state))
                while curr_node.parent is not None:
                    path.append((curr_node.action, curr_node.state))
                    curr_node = curr_node.parent
                path.reverse()
                return path

            if not frontier.contains_state(state) and state not in explored:
                new_node = Node(state, curr_node, action)
                frontier.add(new_node)

def person_id_for_name(name):
    """ Returns the IMDB id for a person's name. """
    person_ids = list(names.get(name.lower(), set()))
    if len(person_ids) == 0:
        return None
    elif len(person_ids) > 1:
        return None
    else:
        return person_ids[0]

def neighbors_for_person(person_id):
    """ Returns (movie_id, person_id) pairs for people who starred with a given person. """
    movie_ids = people[person_id]["movies"]
    neighbors = set()
    for movie_id in movie_ids:
        for person_id in movies[movie_id]["stars"]:
            neighbors.add((movie_id, person_id))
    return neighbors

def find_shortest_path(directory, source_name, target_name):
    """ Find the shortest path between two actors given their names. """
    load_data(directory)
    source = person_id_for_name(source_name)
    target = person_id_for_name(target_name)
    
    if source is None or target is None:
        return "Person not found."

    path = shortest_path(source, target)

    if path is None:
        return "Not connected."
    else:
        degrees = len(path)
        result = f"{degrees} degrees of separation."
        path = [(None, source)] + path
        for i in range(degrees):
            person1 = people[path[i][1]]["name"]
            person2 = people[path[i + 1][1]]["name"]
            movie = movies[path[i + 1][0]]["title"]
            result += f"\n{i + 1}: {person1} and {person2} starred in {movie}"

        return result
