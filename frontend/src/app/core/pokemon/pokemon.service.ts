import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Pokemon } from './pokemon.models';

@Injectable({
  providedIn: 'root'
})
/** * A service that provides methods to interact with the Pokemon API.
 * It allows fetching a list of Pokemon and retrieving details of a specific Pokemon by its ID.
 */
export class PokemonService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = '/api/pokemon';

  getPokemon(limit: number,offset: number): Observable<Pokemon[]> {
    return this.http.get<Pokemon[]>(
      this.apiUrl,
      {
        params: {
          limit,
          offset
        }
      }
    );
  }
}