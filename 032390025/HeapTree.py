class heapTree():
    def __init__(self):
        self.data = []

    def swap(self, idx1, idx2):
        temp = self.data[idx2]
        self.data[idx2] = self.data[idx1]
        self.data[idx1] = temp

    def AddTeam(self, val):
        self.data.append(val)
        self.HeapifyUp(len(self.data) - 1)

    def DelTeam(self, i = 0):
        if len(self.data) <= 0:
            return

        self.data[i] = self.data[-1]
        self.data.pop()

        self.HeapifyDown(i)

    def getData(self):
        for i in range(len(self.data)):
            print(self.data[i].name + "-->", self.data[i].score)

    def getLeftChild(self, i):
        return 2*i + 1
    def getRightChild(self, i):
        return 2*i + 2
    def getParent(self, i):
        return (i-1) // 2

    def haveLeftChild(self, i):
        return self.getLeftChild(i) < len(self.data)
    def haveRigthChild(self, i):
        return self.getRightChild(i) < len(self.data)

    def HeapifyUp(self, i):
        if i <= 0:
            return

        elif self.data[i].score > self.data[self.getParent(i)].score:
            self.swap(i, self.getParent(i))
            return self.HeapifyUp(self.getParent(i))

    def HeapifyDown(self, i):
        if self.haveLeftChild(i):
            greaterChild = self.getLeftChild(i)
            if self.haveRigthChild(i) and (self.data[self.getRightChild(i)].score > self.data[self.getLeftChild(i)].score):
                greaterChild = self.getRightChild(i)
            if self.data[i].score >= self.data[greaterChild].score:
                return
            else:
                self.swap(i, greaterChild)
                return self.HeapifyDown(greaterChild)