from django.urls import path
from . import views

urlpatterns = [
    path("", views.home, name = "home"),
    path("heap_demo/", views.heap_demo, name = "HeapTree")
]