from django.shortcuts import render, HttpResponse
from .models import Team
from .utils.heap_tree import heapTree

# Create your views here.
def home(request):
    return render(request, "home.html")

def heap_demo(request):
    heap = heapTree()

    teams = Team.objects.all()
    for team in teams:
        heap.AddTeam(team)

    if heap.data:
        top_team = heap.data[0]
    else:
        top_team = None

    return render(request, "heap_demo.html", {"top_team": top_team})