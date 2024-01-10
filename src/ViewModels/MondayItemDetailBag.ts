
export type MondayItemDetailBag = {
    name: string;
    createdAt: string;
    updates: MondayUpdateBag[]
    columnValues: AbstractColumnValue[]
}

export type MondayUpdateBag = {
    id: number | string;
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
}

export type BasicColumnValue = {
    id: number | string;
    text: string;
    type: keyof typeof ColumnType;
    value: string;
}

export type Column = {

}

export type FileColumnValue = BasicColumnValue & {
    files: {
        asset_id: string;
        asset: {
            url: string;
            name: string;
            url_thumbnail: string;
            public_url: string;
        };
    }
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
        relativeLink: string;
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