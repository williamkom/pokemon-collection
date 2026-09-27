import {Component,OnInit,computed,inject,signal} from '@angular/core';

import {finalize} from 'rxjs';

import {Router} from '@angular/router';

import {AuthService} from '../../core/auth/auth.service';

import {PokemonService} from '../../core/pokemon/pokemon.service';

import { CollectionService} from '../../core/pokemon/collection.service';

import {CollectionEntry, Pokemon} from '../../core/pokemon/pokemon.models';


@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl:'./home.component.html',
  styleUrl:'./home.component.scss'
})
/** * The HomeComponent is responsible for displaying a list of Pokémon and the user's collection.
 * It allows users to navigate through pages of Pokémon, add Pokémon to their collection, and log out.
 */
export class HomeComponent implements OnInit {

  readonly authService =inject(AuthService);
  private readonly pokemonService =inject(PokemonService);
  private readonly collectionService =inject(CollectionService);
  private readonly router =inject(Router);

  readonly pokemon =signal<Pokemon[]>([]);
  readonly collection =signal<CollectionEntry[]>([]);
  readonly pokemonLoading =signal(false);
  readonly collectionLoading =signal(false);
  readonly addingPokemonId =signal<number | null>(null);
  readonly errorMessage =signal('');
  readonly page =signal(0);
  readonly pageSize = 20;
  // Computed property that returns a Set of Pokémon IDs in the user's collection for quick lookup.
  readonly collectionIds =
    computed(() =>
      new Set(
        this.collection()
          .map(entry =>
            entry.pokemonId
          )
      )
    );


  readonly hasPreviousPage =
    computed(() =>this.page() > 0);

  readonly hasNextPage =
    computed(() =>
      this.pokemon().length ===
      this.pageSize
    );

  ngOnInit(): void {
    this.loadPokemon();
    this.loadCollection();
  }

  /**
   * Loads the list of Pokémon from the API.
   */
  loadPokemon(): void {
    // Set the loading state to true
    this.pokemonLoading.set(true);
    // Clear any previous error message
    this.errorMessage.set('');
    // Calculate the offset based on the current page and page size
    const offset =this.page() *this.pageSize;
    // Call the PokemonService to get the list of Pokémon
    this.pokemonService
      .getPokemon(
        this.pageSize,
        offset
      )
      .pipe(
        finalize(() =>
          this.pokemonLoading.set(false)
        )
      )
      .subscribe({
        next: pokemon => {
          this.pokemon.set(
            pokemon
          );
        },
        error: error => {
          console.error(
            'Could not load Pokémon:',
            error
          );
          this.errorMessage.set(
            'Pokémon could not be loaded.'
          );
        }
      });
  }
  // Loads the user's Pokémon collection from the API.
  loadCollection(): void {
    this.collectionLoading.set(true);
    this.errorMessage.set('');
    this.collectionService
      .getCollection()
      .pipe(
        finalize(() =>
          this.collectionLoading.set(false)
        )
      )
      .subscribe({

        next: collection => {

          this.collection.set(
            collection
          );

        },

        error: error => {

          console.error(
            'Could not load collection:',
            error
          );

          this.errorMessage.set(
            'Your collection could not be loaded.'
          );

        }

      });
  }

  // Adds a Pokémon to the user's collection if it is not already present.
  addPokemon(pokemon: Pokemon): void {
    if (this.collectionIds().has(pokemon.id)
    ) {
      return;
    }

    this.addingPokemonId.set(
      pokemon.id
    );
    this.errorMessage.set('');

    this.collectionService
      .addPokemon(pokemon.id)
      .pipe(
        finalize(() =>this.addingPokemonId.set(null))
      )
      .subscribe({
        next: entry => {
          this.collection.update(
            collection => [
              entry,
              ...collection
            ]
          );
        },
        error: error => {
          console.error(
            'Could not add Pokémon:',
            error
          );
          if (error.status === 409) {
            this.errorMessage.set(
              'This Pokémon is already in your collection.'
            );
            this.loadCollection();
            return;
          }
          if (error.status === 404) {

            this.errorMessage.set(
              'This Pokémon does not exist.'
            );
            return;
          }
          this.errorMessage.set(
            'The Pokémon could not be added.'
          );
        }
      });
  }


  nextPage(): void {
    if (!this.hasNextPage()) {
      return;
    }
    this.page.update(
      page => page + 1
    );
    this.loadPokemon();
  }
  previousPage(): void {

    if (!this.hasPreviousPage()) {
      return;
    }
    this.page.update(
      page => page - 1
    );
    this.loadPokemon();
  }
  isInCollection(
    pokemonId: number
  ): boolean {
    return this.collectionIds()
      .has(pokemonId);
  }
  formatName(
    name: string
  ): string {

    if (!name) {
      return '';
    }
    return (
      name.charAt(0).toUpperCase()
      +
      name.slice(1)
    );
  }
  // Converts the Pokémon's height from decimeters to meters.
  heightInMeters(pokemon: Pokemon): number 
  {
    return pokemon.height / 10;
  }
  // Converts the Pokémon's weight from hectograms to kilograms.
  weightInKilograms(pokemon: Pokemon): number 
  {
    return pokemon.weight / 10;
  }
  
  logout(): void {
    this.authService
      .logout()
      .subscribe({
        next: () =>
          this.router.navigate([
            '/login'
          ]),
        error: () =>
          this.router.navigate([
            '/login'
          ])
      });
  }
}