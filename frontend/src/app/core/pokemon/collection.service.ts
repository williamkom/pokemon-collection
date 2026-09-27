import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CollectionEntry
} from './pokemon.models';

@Injectable({
  providedIn: 'root'
})
/** * A service that provides methods to interact with the Pokemon collection API.
 * It allows fetching the user's collection and adding Pokémon to the collection.
 */
export class CollectionService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl ='/api/collection';

  getCollection():Observable<CollectionEntry[]> {
    return this.http.get<CollectionEntry[]>(
      this.apiUrl
    );
  }
  addPokemon(pokemonId: number): Observable<CollectionEntry> {
    return this.http.post<CollectionEntry>(
      `${this.apiUrl}/${pokemonId}`,
      {}
    );
  }
}