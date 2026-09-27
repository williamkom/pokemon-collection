// This file defines TypeScript interfaces for the Pokemon and CollectionEntry models used in the application.
export interface Pokemon {
  id: number;
  name: string;
  imageUrl: string | null;
  height: number;
  weight: number;
  types: string[];
}

export interface CollectionEntry {
  pokemonId: number;
  addedAt: string;
  pokemon: Pokemon;
}