export interface Book {
    bookId: number,
    title: string,
    author: string,
    genre?: string,
    description?: string,
    averageRating?: number | null,
    createdAt?: string,
    updatedAt?: string,

}