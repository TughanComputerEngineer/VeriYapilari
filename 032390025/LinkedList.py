class Node():
    def __init__(self, oppenent, state):
        self.data = oppenent
        self.state = state
        self.next = None

class linkedList():
    def __init__(self):
        self.head = None

    def getLength(self):
        count = 0
        current = self.head
        while current != None:
            current = current.next
            count+=1
        return count
    def insertAtBegin(self, data, state):
        new_node = Node(data, state)
        new_node.next = self.head
        self.head = new_node
    def insertAtEnd(self, data, state):
        new_node = Node(data, state)
        if self.head == None:
            self.head = new_node
            return

        current = self.head
        while current.next != None:
            current = current.next
        current.next = new_node
    def insertAtIndex(self, data, state, idx = 0):
        if idx < 0 or idx > self.getLength():
            print("invalid index")
            return

        new_node = Node(data, state)

        if self.head == None or idx == 0:
            self.insertAtBegin(data, state)
            return

        current = self.head
        for i in range(idx - 1):
            current = current.next
        tail = current.next

        current.next = new_node
        new_node.next = tail
    def deleteAtIndex(self, idx = None):
        if idx == None:
            idx = self.getLength() - 1

        if idx < 0 or idx >= self.getLength():
            print("invalid index")
            return

        if idx == 0:
            self.head = self.head.next
            return

        current = self.head
        for i in range(idx - 1):
            current = current.next
        current.next = current.next.next
    def deleteAtName(self, name):
        current = self.head

        idx = 0
        while(current != None):
            if (current.data == name):
                self.deleteAtIndex(idx)
                return
            current = current.next
            idx += 1

    def printList(self):
        current = self.head
        while current != None:
            print("Opponent: " + current.data + " State: " + current.state)
            current = current.next