export interface Book {
    bookId: number,
    title: string,
    author: string,
    genre?: string,
    description?: string,
    createdAt?: string,
    updatedAt?: string,
}