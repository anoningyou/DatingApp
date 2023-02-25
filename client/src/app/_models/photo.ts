export interface Photo {
    id: number;
    url: string;
    isMain: boolean;
    isApproved: boolean | null;
    username: string | undefined
}
