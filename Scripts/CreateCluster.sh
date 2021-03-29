cd $(dirname "$0")
cd .. 
k3d cluster create demo -p 5000:80 -v $(pwd)/Source:/Source --no-lb
