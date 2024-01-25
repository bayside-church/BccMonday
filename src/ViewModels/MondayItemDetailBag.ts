
export type MondayItemDetailBag = {
    item: MondayItemBag
    status: string;
    statusIndex: number;
    showApprove: boolean;
    showClose: boolean;
}

export type MondayItemBag = {
    id: string;
    createdAt: string;
    name: string;
    board: MondayBoardBag;
    columnValues: ColumnValueBag[];
    updates: MondayUpdateBag[];
}

export type MondayBoardBag = {
    id: string;
    name: string;
}

export type MondayUpdateBag = {
    id: string;
    createdAt: string;
    creatorName: string;
    textBody: string;
    replies: MondayUpdateBag[] | null;
    files: MondayAssetBag[] | null
}

export type MondayAssetBag = {
    id: number | string;
    name: string;
    publicUrl: string;
}

export type ColumnValueBag = {
    id: number | string;
    text: string;
    type: keyof typeof ColumnType;
    value: string;
    column: Column;
    files?: {
        assetId: string;
        asset: {
            url: string;
            name: string;
            urlThumbnail: string;
            publicUrl: string;
        };
    }[];
    labelStyle?: {
        color: string;
        border: string;
    };
    linkedItemIds?: string[]
}

export type BasicColumnValue = {
    id: number | string;
    text: string;
    type: keyof typeof ColumnType;
    value: string;
    column: Column
}

export type Column = {
    id: string;
    title: string;
}

export type FileColumnValue = BasicColumnValue & {
    files: {
        assetId: string;
        asset: {
            url: string;
            name: string;
            urlThumbnail: string;
            publicUrl: string;
        };
    }[]
}

export type StatusColumnValue = BasicColumnValue & {
    index: number;
    statusLabel: string;
    isDone: boolean;
    labelStyle: {
        color: string;
        border: string;
    }
}

export type BoardRelationColumnValue = BasicColumnValue & {
    displayValue: string;
    linkedItems: {
        id: string;
        name: string;
    }
    linkedItemIds: string[]
}

export type AbstractColumnValue =
    BoardRelationColumnValue
    | FileColumnValue
    | StatusColumnValue
    | BasicColumnValue

export enum ColumnType {
    auto_number,
    board_relation,
    button,
    checkbox,
    color_picker,
    country,
    creation_log,
    date,
    dependency,
    doc,
    dropdown,
    email,
    file,
    formula,
    hour,
    item_assignees,
    item_id,
    last_updated,
    link,
    location,
    long_text,
    mirror,
    name,
    numbers,
    people,
    phone,
    progress,
    rating,
    status,
    subtasks,
    tags,
    team,
    text,
    timeline,
    time_tracking,
    vote,
    week,
    world_clock,
    unsupported,
}