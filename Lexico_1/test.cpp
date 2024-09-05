#include <iostream>
using namespace std;

int main() {
      int suma = 0;
      int input;

      cout << "Ingresa un número: " << endl;
      cin >> input;

      suma += input * 5 - 4 / 3 % 5;

      if((suma >= 0 && suma <= 10) || (suma > $15 && suma < 20)) {
            cout << "Bien";
      }
}